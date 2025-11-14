using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


namespace FullMetalAcorn {
	[ExecuteInEditMode]
	class GroundRenderer : MonoBehaviour {
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 5 * sizeof(uint))]
		struct IndirectDrawArguments {
			public uint indexCountPerInstance;
			public uint instanceCount;
			public uint startIndex;
			public uint baseVertex;
			public uint startInstanceLocation;
		};


		public Mesh groundTileMesh;
		public Material groundTileMaterial;
		
		private Vector2Int cachedSize;

		private ComputeBuffer groundDataBuffer;
		private ComputeBuffer argsBuffer;
		
	
		void Start() {
			this.argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
		}

		void Update() {
			if (this.groundTileMesh == null || this.groundTileMaterial == null || this.groundDataBuffer == null || this.argsBuffer == null) {
				return;
			}

			// if (this.argsBuffer == null) {
			// 	UpdateVisual();
			// }

			Graphics.DrawMeshInstancedIndirect(this.groundTileMesh, 0, this.groundTileMaterial, new Bounds(Vector3.zero, new Vector3(this.cachedSize.x, this.cachedSize.y, 1.0f)), this.argsBuffer);
		}

		public void UpdateVisual(Vector2Int size, List<GroundTile> tiles) {
			if (!this.enabled) {
				return;
			}

			if (size.x < 1) {
				size.x = 1;
			}
			if (size.y < 1) {
				size.y = 1;
			}

			if (this.cachedSize != size || this.groundDataBuffer == null) {
				if (this.groundDataBuffer != null) {
					this.groundDataBuffer.Release();
				}

				if (this.argsBuffer == null) {
					this.argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
				}

				int instanceCount = size.x * size.y;

				this.groundDataBuffer = new ComputeBuffer(instanceCount, sizeof(uint));

				IndirectDrawArguments args;
				args.indexCountPerInstance = groundTileMesh.GetIndexCount(0);
				args.instanceCount = (uint) instanceCount;
				args.startIndex = groundTileMesh.GetIndexStart(0);
				args.baseVertex = groundTileMesh.GetBaseVertex(0);
				args.startInstanceLocation = 0;

				uint[] bep = new uint[5];
				bep[0] = args.indexCountPerInstance;
				bep[1] = args.instanceCount;
				bep[2] = args.startIndex;
				bep[3] = args.baseVertex;
				bep[4] = args.startInstanceLocation;
				
				this.argsBuffer.SetData(bep);
				
				this.groundTileMaterial.SetVector("_GroundSize", new Vector4(size.x, size.y, 0, 0));
			}

			this.cachedSize = size;
			
			uint[] tileDataBuffer = new uint[Mathf.Min(size.x * size.y, tiles.Count)];

			for (int i = 0; i < tileDataBuffer.Length; i++) {
				tileDataBuffer[i] = tiles[i].highlighted ? 1u : 0u;
			}

			this.groundDataBuffer.SetData(tileDataBuffer, 0, 0, tileDataBuffer.Length);

			this.groundTileMaterial.SetBuffer("dataBuffer", this.groundDataBuffer);
		}

		void OnDisable() {
			if (this.groundDataBuffer != null) {
				this.groundDataBuffer.Release();
			}
			this.groundDataBuffer = null;

			if (this.argsBuffer != null)
				this.argsBuffer.Release();
			this.argsBuffer = null;
		}
	}
}